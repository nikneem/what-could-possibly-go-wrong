import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { catchError, map, of, Subscription } from 'rxjs';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { IQuestion, ISurvey } from '@shared-state/survey/survey.models';
import { Store } from '@ngrx/store';
import { IAppState } from '@shared-state/app.state';
import { SurveyActions } from '@shared-state/survey/survey.actions';
import { RealtimeService } from '@services/realtime.service';
import { ClientService } from '@services/client.service';
import { IVoteCreateRequest } from '@shared-state/models/votr.models';
import { VotesService } from '@services/votes.service';

@Component({
  selector: 'wcpgw-votes-landing-page',
  standalone: false,
  templateUrl: './votes-landing-page.component.html',
  styleUrl: './votes-landing-page.component.scss',
})
export class VotesLandingPageComponent implements OnInit, OnDestroy {
  pageError?: string;
  errorMessage?: string;
  surveyCode?: string;

  isLoading: boolean = false;
  survey?: ISurvey;
  activeQuestion?: IQuestion;
  private surveysSubscription: Subscription;

  form?: FormGroup;
  private clientId: string;

  success = false;

  constructor(
    private activatedRoute: ActivatedRoute,
    private realtimeService: RealtimeService,
    private clientService: ClientService,
    private votesService: VotesService,
    private store: Store<IAppState>
  ) {
    this.clientId = this.clientService.getClientId();
    this.surveysSubscription = this.store
      .select((str) => str.surveys)
      .subscribe((state) => {
        this.isLoading = state.isLoading;
        this.errorMessage = state.errorMessage;
        this.survey = state.survey;

        if (state.survey) {
          if (this.surveyCode !== state.survey.code) {
            this.realtimeService.connect(state.survey.code, this.clientId);
            this.findActiveQuestion();
          }
        }
        if (
          state.activeQuestion &&
          state.activeQuestion.id !== this.activeQuestion?.id
        ) {
          this.activateQuestion(state.activeQuestion);
        }

        this.surveyCode = state.survey?.code;
        this.activeQuestion = state.activeQuestion;
      });
  }

  private loadSurvey(code: string) {
    if (this.surveyCode !== code) {
      this.store.dispatch(SurveyActions.surveyLoad({ surveyCode: code }));
    }
  }

  private findActiveQuestion() {
    if (this.survey) {
      const activeQuestion = this.survey.questions.find(
        (question) => question.isActive
      );
      if (activeQuestion) {
        this.store.dispatch(
          SurveyActions.questionActivated({ question: activeQuestion })
        );
      }
    }
    return undefined;
  }
  private activateQuestion(question: IQuestion) {
    if (this.survey) {
      this.form = new FormGroup({
        surveyId: new FormControl(this.survey.id, [Validators.required]),
        questionId: new FormControl(question.id, [Validators.required]),
        answerId: new FormControl(null, [Validators.required]),
      });
    }
  }

  submitAnswer() {
    if (this.form?.valid && !this.form?.pristine) {
      const requestPayload = this.form.value as IVoteCreateRequest;
      this.votesService
        .castVote(this.clientId, requestPayload)
        .pipe(
          map((response) => response.ok),
          catchError((error) => of(false))
        )
        .subscribe((success) => {
          this.success = success;
          if (this.success) {
            this.form?.markAsPristine();
            setTimeout(() => (this.success = false), 5000);
          }
        });
    }
  }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params) => {
      const queryStringSurveyCode = params['code'];
      if (queryStringSurveyCode) {
        this.loadSurvey(queryStringSurveyCode);
      }
    });
  }

  ngOnDestroy(): void {
    this.surveysSubscription.unsubscribe();
  }
}
