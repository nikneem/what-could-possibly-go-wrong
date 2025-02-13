import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { MaterialModule } from '../material/material.module';
import { FooterComponent } from './footer/footer.component';
import { GraphProgressBarComponent } from './graph-progress-bar/graph-progress-bar.component';
import { PageErrorComponent } from './page-error/page-error.component';

@NgModule({
  declarations: [
    ToolbarComponent,
    FooterComponent,
    GraphProgressBarComponent,
    PageErrorComponent,
  ],
  imports: [CommonModule, MaterialModule],
  exports: [
    ToolbarComponent,
    FooterComponent,
    GraphProgressBarComponent,
    PageErrorComponent,
  ],
})
export class ComponentsModule {}
