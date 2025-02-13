import { Component, Input } from '@angular/core';

@Component({
  selector: 'wcpgw-page-error',
  standalone: false,
  templateUrl: './page-error.component.html',
  styleUrl: './page-error.component.scss',
})
export class PageErrorComponent {
  @Input() title?: string;
  @Input() text: string = 'An error occurred';
}
