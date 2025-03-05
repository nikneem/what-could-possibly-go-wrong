import { HttpClient } from '@angular/common/http';
import { Injectable, signal, WritableSignal } from '@angular/core';
import { environment } from '../../environments/environment';
import {
  IQuestionVotes,
  IRealtimeConnectionResponse,
} from '@shared-state/models/votr.models';
import { IQuestion } from '@shared-state/survey/survey.models';
import { Store } from '@ngrx/store';
import { IAppState } from '@shared-state/app.state';
import { SurveyActions } from '@shared-state/survey/survey.actions';
import { Observable } from 'rxjs';

export interface IRealtimeEvent<TPayload> {
  messageType: string;
  payload: TPayload;
}

@Injectable({
  providedIn: 'root',
})
export class RealtimeService {
  private baseUrl: string;
  private ackCounter: number = 0;
  public pubsubClient?: WebSocket;
  private surveyCode?: string;

  votesReceived: WritableSignal<IQuestionVotes | null> = signal(null);

  constructor(private http: HttpClient, private store: Store<IAppState>) {
    this.baseUrl = environment.apiEndpoint;
  }

  public connect(surveyCode: string, voterId: string) {
    if (
      this.pubsubClient &&
      this.pubsubClient.readyState === this.pubsubClient.OPEN
    ) {
      console.log('Connection already open, doing nothing');
    } else {
      this.surveyCode = surveyCode;
      var url = `${this.baseUrl}/surveys/${surveyCode}/connect`;
      this.http
        .get<IRealtimeConnectionResponse>(url, {
          headers: {
            'X-VotR-VoterId': voterId,
          },
        })
        .subscribe((val) => {
          this.connectSocket(val.webPubsubEndpointUrl);
        });
    }
  }

  private connectSocket(url: string) {
    const self = this;
    if (this.surveyCode) {
      this.pubsubClient = new WebSocket(url, 'json.webpubsub.azure.v1');
      this.pubsubClient.onopen = (evt) => {
        console.log(`Realtime connected to session ${self.surveyCode}`);
        if (self.surveyCode) {
          this.joinGroup(self.surveyCode);
        }
      };

      this.pubsubClient.onmessage = (msg) => {
        self.handleRealtimeMessage(msg.data);
      };
    }
  }

  public joinGroup(groupName: string) {
    if (this.pubsubClient?.readyState === this.pubsubClient?.OPEN) {
      console.log(`Joining group ${groupName}`);
      this.pubsubClient?.send(
        JSON.stringify({
          type: 'joinGroup',
          group: groupName,
          ackId: this.getAckId(),
        })
      );
    }
  }
  public leaveGroup(groupName: string) {
    if (this.pubsubClient?.readyState === this.pubsubClient?.OPEN) {
      console.log(`Joining group ${groupName}`);
      this.pubsubClient?.send(
        JSON.stringify({
          type: 'joinGroup',
          group: groupName,
          ackId: this.getAckId(),
        })
      );
    }
  }

  private getAckId(): number {
    this.ackCounter++;
    return this.ackCounter;
  }

  private handleRealtimeMessage(message: any) {
    var eventMessage = JSON.parse(message);
    if (eventMessage.type === 'message') {
      const event = eventMessage.data as IRealtimeEvent<any>;
      if (event.messageType === 'SurveyQuestionActivated') {
        const activatedPoll = event.payload as IQuestion;
        this.store.dispatch(
          SurveyActions.questionActivated({ question: activatedPoll })
        );
        //this.store.dispatch(pollActivated({ poll: activatedPoll }));
      }
      if (event.messageType === 'SurveyQuestionVotesChanged') {
        const questionVotes = event.payload as IQuestionVotes;
        this.handleVotesReceived(questionVotes);
        //this.store.dispatch(pollActivated({ poll: activatedPoll }));
      }
      // if (event.eventName === 'poll-votes') {
      //   const incomingVotes = event.payload as Array<IPollVoteDto>;
      //   this.store.dispatch(voteSeriesUpdate({ dto: incomingVotes }));
      // }
    }
  }

  private handleVotesReceived(votes: IQuestionVotes) {
    this.votesReceived.update((v) => votes);
  }

  //  webPubsubEndpointUrl;
}
