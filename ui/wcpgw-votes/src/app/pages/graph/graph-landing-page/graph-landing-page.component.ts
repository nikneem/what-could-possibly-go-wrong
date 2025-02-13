import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, map, catchError, of } from 'rxjs';
import { ISurvey, IQuestion } from '../../../models/survey.models';
import { SurveysService } from '../../../services/surveys.service';

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
          } else {
            this.pageError = 'Survey not found';
          }
        });
    }
  }
  private findActiveQuestion() {
    if (this.survey) {
      this.activeQuestion = this.survey.questions.find(
        (question) => question.isActive
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
