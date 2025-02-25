import { HttpClient, HttpResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { IVoteCreateRequest } from '@shared-state/models/votr.models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class VotesService {
  private baseUrl: string;
  constructor(private http: HttpClient) {
    this.baseUrl = environment.apiEndpoint;
  }

  public castVote(
    requestData: IVoteCreateRequest
  ): Observable<HttpResponse<Response>> {
    const url = `${this.baseUrl}/votes`;
    return this.http.post<Response>(url, requestData, { observe: 'response' });
  }
}
