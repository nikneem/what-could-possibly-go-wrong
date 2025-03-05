import { Component, effect, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { RealtimeService } from '@services/realtime.service';
import { SurveysService } from '@services/surveys.service';
import { IAppState } from '@shared-state/app.state';
import { IQuestionVotes } from '@shared-state/models/votr.models';
import { SurveyActions } from '@shared-state/survey/survey.actions';
import { IQuestion, ISurvey } from '@shared-state/survey/survey.models';
import { Subscription, map, catchError, of } from 'rxjs';

@Component({
  selector: 'wcpgw-graph-landing-page',
  standalone: false,
  templateUrl: './graph-landing-page.component.html',
  styleUrl: './graph-landing-page.component.scss',
})
export class GraphLandingPageComponent implements OnInit, OnDestroy {
  pageError?: string;
  errorMessage?: string;
  isLoading: boolean = false;
  survey?: ISurvey;
  activeQuestion?: IQuestion;
  surveyCode?: string;

  private surveysSubscription?: Subscription;

  form?: FormGroup;
  votes: IQuestionVotes | null = null;

  constructor(
    private activatedRoute: ActivatedRoute,
    private realtimeService: RealtimeService,
    private store: Store<IAppState>
  ) {
    effect(() => {
      this.votes = this.realtimeService.votesReceived();
    });

    this.surveysSubscription = this.store
      .select((str) => str.surveys)
      .subscribe((state) => {
        this.isLoading = state.isLoading;
        this.errorMessage = state.errorMessage;
        this.survey = state.survey;
        if (state.survey) {
          if (this.surveyCode !== state.survey.code) {
            this.realtimeService.connect(
              state.survey.code,
              '00000000-0000-0000-0000-000000000001'
            );
            this.findActiveQuestion();
          }
        }
        if (
          state.activeQuestion &&
          state.activeQuestion.id !== this.activeQuestion?.id
        ) {
          this.activeQuestion = state.activeQuestion;
        }

        this.surveyCode = state.survey?.code;
      });
  }

  getProgressFor(optionId: string): number {
    if (this.votes) {
      const answer = this.votes.answers.find((a) => a.answerId === optionId);
      if (answer) {
        return answer.percentage * 100;
      }
    }
    return 0;
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

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params) => {
      const surveyCode = params['code'];
      if (surveyCode) {
        this.loadSurvey(surveyCode);
      } else {
        this.pageError = 'Survey not found';
      }
    });
  }

  ngOnDestroy(): void {}
}
