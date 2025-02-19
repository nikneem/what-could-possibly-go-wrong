import { Component, OnDestroy } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { SurveysService } from '../../../services/surveys.service';
import { catchError, map, of, Subscription } from 'rxjs';
import { Router } from '@angular/router';
import { ISurvey } from '@shared-state/survey/survey.models';
import { Store } from '@ngrx/store';
import { IAppState } from '@shared-state/app.state';
import { SurveyActions } from '@shared-state/survey/survey.actions';

@Component({
  selector: 'wcpgw-home-landing-page',
  standalone: false,
  templateUrl: './home-landing-page.component.html',
  styleUrl: './home-landing-page.component.scss',
})
export class HomeLandingPageComponent implements OnDestroy {
  form: FormGroup;
  isLoading: boolean = false;
  errorMessage?: string;

  private surveysSubscription: Subscription;

  constructor(private router: Router, private store: Store<IAppState>) {
    this.form = new FormGroup({
      code: new FormControl('', [Validators.required, Validators.minLength(6)]),
    });

    this.surveysSubscription = this.store
      .select((str) => str.surveys)
      .subscribe((state) => {
        this.isLoading = state.isLoading;
        this.errorMessage = state.errorMessage;
        if (state.survey) {
          this.router.navigate(['/votes', state.survey.code]);
        }
      });
  }

  joinSurvey() {
    if (this.form.valid) {
      this.store.dispatch(
        SurveyActions.surveyLoad({ surveyCode: this.form.value.code })
      );
    }
  }

  ngOnDestroy() {
    this.surveysSubscription.unsubscribe();
  }
}
