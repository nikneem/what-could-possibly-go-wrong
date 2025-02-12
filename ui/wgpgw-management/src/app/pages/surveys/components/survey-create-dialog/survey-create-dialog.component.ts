import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

import {
  ISurvey,
  ISurveyCreateRequest,
} from '../../../../models/survey.models';
import { catchError, map, of, Subscription } from 'rxjs';
import { SurveysService } from '../../../../services/surveys.service';
import { MatDialogRef } from '@angular/material/dialog';
import * as _moment from 'moment';
import { default as _rollupMoment } from 'moment';
const moment = _rollupMoment || _moment;
@Component({
  selector: 'wgpgw-survey-create-dialog',
  standalone: false,
  templateUrl: './survey-create-dialog.component.html',
  styleUrl: './survey-create-dialog.component.scss',
})
export class SurveyCreateDialogComponent {
  form: FormGroup;
  isLoading: boolean = false;
  private surveysSubscription?: Subscription;

  constructor(
    private surveysService: SurveysService,
    private dialogRef: MatDialogRef<SurveyCreateDialogComponent>
  ) {
    const twoMonthsFromNow = moment().add(2, 'months');
    this.form = new FormGroup({
      name: new FormControl('', [Validators.required]),
      expiresOn: new FormControl(twoMonthsFromNow, [Validators.required]),
    });
  }

  private createSurvey(payload: ISurveyCreateRequest): void {
    this.surveysSubscription?.unsubscribe();
    if (!this.isLoading) {
      this.isLoading = true;
      this.surveysSubscription = this.surveysService
        .createSurvey(payload)
        .pipe(
          map((response) => response.data),
          catchError((error) => {
            console.error(error);
            return of(null);
          })
        )
        .subscribe((survey: ISurvey | null) => {
          this.isLoading = false;
          if (survey) {
            this.dialogRef.close(survey);
          }
        });
    }
  }

  save() {
    if (this.form.valid) {
      const payload: ISurveyCreateRequest = this.form.value;
      this.createSurvey(payload);
    }
  }
}
