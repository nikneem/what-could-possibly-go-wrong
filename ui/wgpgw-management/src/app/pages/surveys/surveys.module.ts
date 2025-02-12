import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SurveysRoutingModule } from './surveys-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { SurveyListPageComponent } from './survey-list-page/survey-list-page.component';
import { SurveyCreateDialogComponent } from './components/survey-create-dialog/survey-create-dialog.component';
import { ReactiveFormsModule } from '@angular/forms';
import { provideNativeDateAdapter } from '@angular/material/core';
import { provideMomentDateAdapter } from '@angular/material-moment-adapter';
import { SurveyDetailsPageComponent } from './survey-details-page/survey-details-page.component';

@NgModule({
  declarations: [SurveyListPageComponent, SurveyCreateDialogComponent, SurveyDetailsPageComponent],
  imports: [
    CommonModule,
    SurveysRoutingModule,
    SharedModule,
    ReactiveFormsModule,
  ],
  providers: [provideMomentDateAdapter()],
})
export class SurveysModule {}
