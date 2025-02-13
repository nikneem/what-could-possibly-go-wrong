import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Subscription, map, catchError, of } from 'rxjs';
import { IQuestion, ISurvey } from '../../../models/survey.models';
import { SurveysService } from '../../../services/surveys.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'wcpgw-votes-landing-page',
  standalone: false,
  templateUrl: './votes-landing-page.component.html',
  styleUrl: './votes-landing-page.component.scss',
})
export class VotesLandingPageComponent implements OnInit, OnDestroy {
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
      if (this.activeQuestion) {
        this.form = new FormGroup({
          code: new FormControl(this.survey.code, [
            Validators.required,
            Validators.minLength(6),
          ]),
          questionId: new FormControl(this.activeQuestion.id, [
            Validators.required,
          ]),
          answerId: new FormControl(null, [Validators.required]),
        });
      }
    }
    return undefined;
  }

  submitAnswer() {}

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
