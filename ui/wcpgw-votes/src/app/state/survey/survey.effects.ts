import { inject, Injectable } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { SurveyActions } from './survey.actions';
import { catchError, exhaustMap, map, of } from 'rxjs';
import { SurveysService } from '../../services/surveys.service';

@Injectable()
export class SurveysEffects {
  private actions$ = inject(Actions);
  private surveysService = inject(SurveysService);

  surveyLoadEffect$ = createEffect(() => {
    return this.actions$.pipe(
      ofType(SurveyActions.surveyLoad),
      exhaustMap((act) =>
        this.surveysService.getSurvey(act.surveyCode).pipe(
          map((survey) =>
            survey.isSuccess
              ? SurveyActions.surveyLoaded({ survey: survey.data })
              : SurveyActions.surveyLoadError({
                  errorMessage: survey.errorMessage,
                })
          ),
          catchError(() =>
            of(
              SurveyActions.surveyLoadError({
                errorMessage: 'Failed to load survey',
              })
            )
          )
        )
      )
    );
  });
}
