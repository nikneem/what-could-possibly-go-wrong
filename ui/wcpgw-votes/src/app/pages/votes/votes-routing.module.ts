import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { VotesLandingPageComponent } from './votes-landing-page/votes-landing-page.component';

const routes: Routes = [
  { path: '', component: VotesLandingPageComponent },
  { path: ':code', component: VotesLandingPageComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class VotesRoutingModule {}
