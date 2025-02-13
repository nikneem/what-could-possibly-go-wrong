import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  {
    path: 'home',
    loadChildren: () =>
      import('./pages/home/home.module').then((m) => m.HomeModule),
  },
  {
    path: 'votes',
    loadChildren: () =>
      import('./pages/votes/votes.module').then((m) => m.VotesModule),
  },
  {
    path: 'graph',
    loadChildren: () =>
      import('./pages/graph/graph.module').then((m) => m.GraphModule),
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}
