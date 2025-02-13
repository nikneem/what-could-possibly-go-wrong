import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { GraphLandingPageComponent } from './graph-landing-page/graph-landing-page.component';

const routes: Routes = [
  { path: '', component: GraphLandingPageComponent },
  { path: ':code', component: GraphLandingPageComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class GraphRoutingModule {}
