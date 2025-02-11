import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ToolbarComponent } from './toolbar/toolbar.component';
import { MaterialModule } from '../material/material.module';
import { EmptyStateComponent } from './empty-state/empty-state.component';

@NgModule({
  declarations: [ToolbarComponent, EmptyStateComponent],
  imports: [CommonModule, MaterialModule],
  exports: [ToolbarComponent, EmptyStateComponent],
})
export class ComponentsModule {}
