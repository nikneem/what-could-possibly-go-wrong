import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SurveysRoutingModule } from './surveys-routing.module';
import { SharedModule } from '../../shared/shared.module';
import { SurveyListPageComponent } from './survey-list-page/survey-list-page.component';

@NgModule({
  declarations: [
    SurveyListPageComponent
  ],
  imports: [CommonModule, SurveysRoutingModule, SharedModule],
})
export class SurveysModule {}
