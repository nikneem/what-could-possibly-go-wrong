import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SurveyListPageComponent } from './survey-list-page/survey-list-page.component';
import { SurveyDetailsPageComponent } from './survey-details-page/survey-details-page.component';

const routes: Routes = [
  { path: '', component: SurveyListPageComponent },
  { path: ':code', component: SurveyDetailsPageComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class SurveysRoutingModule {}
