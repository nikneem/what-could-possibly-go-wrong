import { IQuestion, ISurvey } from './survey.models';

export interface ISurveyState {
  survey?: ISurvey;
  activeQuestion?: IQuestion;
  isLoading: boolean;
  errorMessage?: string;
}

export const initialSurveyState: ISurveyState = {
  isLoading: false,
};
