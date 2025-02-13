import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { VotesRoutingModule } from './votes-routing.module';
import { VotesLandingPageComponent } from './votes-landing-page/votes-landing-page.component';
import { SharedModule } from '../../shared/shared.module';
import { ReactiveFormsModule } from '@angular/forms';

@NgModule({
  declarations: [VotesLandingPageComponent],
  imports: [
    CommonModule,
    VotesRoutingModule,
    SharedModule,
    ReactiveFormsModule,
  ],
})
export class VotesModule {}
