import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { IRealtimeConnectionResponse } from '../models/votr.models';

export interface IRealtimeEvent<TPayload> {
  message: string;
  data: TPayload;
}

@Injectable({
  providedIn: 'root',
})
export class RealtimeService {
  private baseUrl: string;
  private ackCounter: number = 0;
  public pubsubClient?: WebSocket;
  private surveyCode?: string;

  constructor(private http: HttpClient) {
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
      // var event = eventMessage.data as IRealtimeEvent<any>;
      console.log(event);
      // if (event.eventName === 'poll-activated') {
      //   const activatedPoll = event.payload as IPollDto;
      //   this.store.dispatch(pollActivated({ poll: activatedPoll }));
      // }
      // if (event.eventName === 'poll-votes') {
      //   const incomingVotes = event.payload as Array<IPollVoteDto>;
      //   this.store.dispatch(voteSeriesUpdate({ dto: incomingVotes }));
      // }
    }
  }

  //  webPubsubEndpointUrl;
}
