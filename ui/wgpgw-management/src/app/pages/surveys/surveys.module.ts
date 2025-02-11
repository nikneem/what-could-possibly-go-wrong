import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SurveysRoutingModule } from './surveys-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { SurveyListPageComponent } from './survey-list-page/survey-list-page.component';
import { SurveyCreateDialogComponent } from './components/survey-create-dialog/survey-create-dialog.component';
import { ReactiveFormsModule } from '@angular/forms';
import { provideNativeDateAdapter } from '@angular/material/core';

@NgModule({
  declarations: [SurveyListPageComponent, SurveyCreateDialogComponent],
  imports: [
    CommonModule,
    SurveysRoutingModule,
    SharedModule,
    ReactiveFormsModule,
  ],
  providers: [provideNativeDateAdapter()],
})
export class SurveysModule {}
