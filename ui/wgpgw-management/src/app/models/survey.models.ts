export interface ISurvey {
  id: number;
  name: string;
  code: string;
  expiresOn: Date;
  questions: IQuestion[];
}

export interface IQuestion {
  id: number;
  text: string;
  order: number;
  answerOptions: IOption[];
}

export interface IOption {
  id: string;
  text: string;
  order: number;
}
