export interface ISurvey {
  id: number;
  name: string;
  code: string;
  expiresOn: Date;
  questions: IQuestion[];
}

export interface IQuestion {
  id: string;
  text: string;
  order: number;
  answerOptions: IOption[];
}

export interface IOption {
  id: string;
  text: string;
  order: number;
}

export interface ISurveyCreateRequest {
  name: string;
  expiresOn: Date;
}

export interface ISurveyUpdateRequest {
  name: string;
  expiresOn: Date;
  questions: ISurveyUpdateQuestion[];
}

export interface ISurveyUpdateQuestion {
  id?: string;
  text: string;
  order: number;
  answers: ISurveyUpdateAnswer[];
}

export interface ISurveyUpdateAnswer {
  id?: string;
  text: string;
  order: number;
}
