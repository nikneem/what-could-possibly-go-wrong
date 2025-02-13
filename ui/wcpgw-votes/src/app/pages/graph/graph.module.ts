import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { GraphRoutingModule } from './graph-routing.module';
import { GraphLandingPageComponent } from './graph-landing-page/graph-landing-page.component';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  declarations: [GraphLandingPageComponent],
  imports: [CommonModule, GraphRoutingModule, SharedModule],
})
export class GraphModule {}
