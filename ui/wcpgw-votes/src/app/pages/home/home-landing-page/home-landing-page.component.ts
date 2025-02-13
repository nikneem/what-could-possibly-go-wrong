import { Component } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SurveysService } from '../../../services/surveys.service';
import { catchError, map, of, Subscription } from 'rxjs';
import { ISurvey } from '../../../models/survey.models';
import { Router } from '@angular/router';

@Component({
  selector: 'wcpgw-home-landing-page',
  standalone: false,
  templateUrl: './home-landing-page.component.html',
  styleUrl: './home-landing-page.component.scss',
})
export class HomeLandingPageComponent {
  form: FormGroup;
  isLoading: boolean = false;
  errorMessage?: string;

  private surveysSubscription?: Subscription;

  constructor(private surveysService: SurveysService, private router: Router) {
    this.form = new FormGroup({
      code: new FormControl('', [Validators.required, Validators.minLength(6)]),
    });
  }

  private loadSurvey(code: string) {
    this.surveysSubscription?.unsubscribe();
    if (!this.isLoading) {
      this.errorMessage = undefined;
      this.isLoading = true;
      this.surveysSubscription = this.surveysService
        .getSurvey(code)
        .pipe(
          map((response) => response.data),
          catchError((error) => {
            console.error(error);
            return of(null);
          })
        )
        .subscribe((survey: ISurvey | null | undefined) => {
          this.isLoading = false;
          if (survey) {
            this.router.navigate(['/votes', survey.code]);
          } else {
            this.errorMessage = 'Survey not found';
          }
        });
    }
  }

  joinSurvey() {
    if (this.form.valid) {
      this.loadSurvey(this.form.value.code);
    }
  }
}
