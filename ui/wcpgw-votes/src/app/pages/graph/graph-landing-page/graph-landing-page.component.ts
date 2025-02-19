import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { RealtimeService } from '@services/realtime.service';
import { SurveysService } from '@services/surveys.service';
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
  private surveysSubscription?: Subscription;

  form?: FormGroup;

  constructor(
    private activatedRoute: ActivatedRoute,
    private surveysService: SurveysService,
    private realtimeService: RealtimeService,
    private router: Router
  ) {}

  private loadSurvey(code: string) {
    this.surveysSubscription?.unsubscribe();
    if (!this.isLoading) {
      this.pageError = undefined;
      this.isLoading = true;
      this.surveysSubscription = this.surveysService
        .getSurvey(code)
        .pipe(
          map((response) => response.data),
          catchError((error) => {
            console.error(error);
            return of(null);
          })
        )
        .subscribe((survey: ISurvey | null | undefined) => {
          this.isLoading = false;
          if (survey) {
            this.survey = survey;
            this.findActiveQuestion();
            this.connectRealtimeService();
          } else {
            this.pageError = 'Survey not found';
          }
        });
    }
  }
  private findActiveQuestion() {
    if (this.survey) {
      this.activeQuestion = this.survey.questions.find(
        (question: IQuestion) => question.isActive
      );
    }
  }
  private connectRealtimeService() {
    if (this.survey) {
      this.realtimeService.connect(
        this.survey.code,
        '00000000-0000-0000-0000-000000000001'
      );
    }
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
