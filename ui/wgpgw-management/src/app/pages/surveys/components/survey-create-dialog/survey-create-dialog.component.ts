import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'wgpgw-survey-create-dialog',
  standalone: false,
  templateUrl: './survey-create-dialog.component.html',
  styleUrl: './survey-create-dialog.component.scss',
})
export class SurveyCreateDialogComponent {
  form: FormGroup;

  constructor() {
    this.form = new FormGroup({
      name: new FormControl('', [Validators.required]),
      expiresOn: new FormControl('', [Validators.required]),
    });
  }
}
