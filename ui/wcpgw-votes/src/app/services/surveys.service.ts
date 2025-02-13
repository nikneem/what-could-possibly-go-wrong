import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { ISurvey } from '../models/survey.models';
import { IResponseModel } from '../models/votr.models';

@Injectable({
  providedIn: 'root',
})
export class SurveysService {
  private baseUrl: string;
  constructor(private http: HttpClient) {
    this.baseUrl = environment.apiEndpoint;
  }

  public getSurveys(): Observable<IResponseModel<Array<ISurvey>>> {
    const url = `${this.baseUrl}/surveys`;
    return this.http.get<IResponseModel<Array<ISurvey>>>(url);
  }

  public getSurvey(code: string): Observable<IResponseModel<ISurvey>> {
    const url = `${this.baseUrl}/surveys/${code}`;
    return this.http.get<IResponseModel<ISurvey>>(url);
  }
}
