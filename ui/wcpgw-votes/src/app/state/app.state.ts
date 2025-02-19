import { surveysReducer } from './survey/survey.reducer';
import { initialSurveyState, ISurveyState } from './survey/survey.state';

export interface IAppState {
  surveys: ISurveyState;
}
export const initialAppState: IAppState = {
  surveys: initialSurveyState,
};
export const reducers = {
  surveys: surveysReducer,
};
