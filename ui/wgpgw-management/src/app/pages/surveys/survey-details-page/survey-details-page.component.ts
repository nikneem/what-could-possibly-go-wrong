import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormArray, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, map, of, Subscription } from 'rxjs';
import { SurveysService } from '../../../services/surveys.service';
import { ISurvey, ISurveyUpdateRequest } from '../../../models/survey.models';

@Component({
  selector: 'wgpgw-survey-details-page',
  standalone: false,
  templateUrl: './survey-details-page.component.html',
  styleUrl: './survey-details-page.component.scss',
})
export class SurveyDetailsPageComponent implements OnInit, OnDestroy {
  isLoading: boolean = false;
  form: FormGroup;
  private surveyCode?: string;
  private survey?: ISurvey;
  private surveysSubscription?: Subscription;
  private surveyUpdateSubscription?: Subscription;

  constructor(
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private surveysService: SurveysService
  ) {
    this.form = new FormGroup({});
    this.createForm();
  }

  private createForm(): void {
    this.form = new FormGroup({
      name: new FormControl('', [Validators.required, Validators.minLength(5)]),
      expiresOn: new FormControl('', [Validators.required]),
      questions: new FormArray([]),
    });
  }

  private populateForm() {
    if (this.survey) {
      this.form.patchValue(this.survey);
      this.survey.questions.forEach((question, questionIndex) => {
        let questionForm = this.addQuestion();
        questionForm.patchValue(question);
        question.answerOptions.forEach((answer) => {
          let answerForm = this.addAnswer(questionIndex);
          answerForm.patchValue(answer);
        });
      });
      this.form.markAsPristine();
    }
  }

  private loadSurvey(code: string): void {
    this.surveysSubscription?.unsubscribe();
    if (!this.isLoading) {
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
            this.populateForm();
          }
        });
    }
  }
  private updateSurvey(payload: ISurveyUpdateRequest): void {
    if (this.surveyCode) {
      this.surveyUpdateSubscription?.unsubscribe();
      if (!this.isLoading) {
        this.isLoading = true;
        this.surveyUpdateSubscription = this.surveysService
          .updateSurvey(this.surveyCode, payload)
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
              this.createForm();
              this.populateForm();
            }
          });
      }
    }
  }

  get questions(): FormArray<FormGroup> {
    return this.form.get('questions') as FormArray;
  }

  answers(index: number): FormArray<FormGroup> {
    var questionsFormArray = this.form.get('questions') as FormArray;
    var questionForm = questionsFormArray.at(index) as FormGroup;
    return questionForm.get('answers') as FormArray<FormGroup>;
  }

  addQuestion(): FormGroup {
    const order = this.questions.length;
    let questionForm = new FormGroup({
      id: new FormControl(null),
      text: new FormControl('', [Validators.required]),
      order: new FormControl(order, [Validators.required]),
      answers: new FormArray([]),
    });

    this.questions.push(questionForm);
    this.form.markAsDirty();
    return questionForm;
  }
  removeQuestion(index: number) {
    this.questions.removeAt(index);
    this.form.markAsDirty();
  }
  addAnswer(index: number): FormGroup {
    const order = this.answers(index).length;
    let answerForm = new FormGroup({
      id: new FormControl(null),
      text: new FormControl('', [Validators.required]),
      order: new FormControl(order, [Validators.required]),
    });
    this.answers(index).push(answerForm);
    this.form.markAsDirty();
    return answerForm;
  }
  removeAnswer(questionIndex: number, answerIndex: number) {
    this.answers(questionIndex).removeAt(answerIndex);
    this.form.markAsDirty();
  }

  activateQuestion(index: number) {
    if (this.surveyCode) {
      const question = this.questions.at(index);
      if (question) {
        this.surveyUpdateSubscription = this.surveysService
          .activateSurveyQuestion(this.surveyCode, question.value.id)
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
              this.form.markAsPristine();
            }
          });
      }
    }
  }

  save() {
    if (this.form.valid && this.form.dirty) {
      var requestPayload = this.form.value as ISurveyUpdateRequest;
      this.updateSurvey(requestPayload);
    }
  }
  back() {
    this.router.navigate(['/surveys']);
  }

  ngOnInit(): void {
    this.activatedRoute.params.subscribe((params) => {
      this.surveyCode = params['code'];
      if (this.surveyCode) {
        this.loadSurvey(this.surveyCode);
      }
    });
  }

  ngOnDestroy(): void {
    this.surveysSubscription?.unsubscribe();
    this.surveyUpdateSubscription?.unsubscribe();
  }
}
