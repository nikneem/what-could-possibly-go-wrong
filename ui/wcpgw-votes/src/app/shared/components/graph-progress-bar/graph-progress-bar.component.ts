import { Component, Input } from '@angular/core';

@Component({
  selector: 'wcpgw-graph-progress-bar',
  standalone: false,
  templateUrl: './graph-progress-bar.component.html',
  styleUrl: './graph-progress-bar.component.scss',
})
export class GraphProgressBarComponent {
  @Input() progress: number = 0;

  getProgressPercentage(): string {
    return `${this.progress}%`;
  }
}
