import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: 'surveys', pathMatch: 'full' },
  {
    path: 'surveys',
    loadChildren: () =>
      import('./pages/surveys/surveys.module').then((m) => m.SurveysModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
