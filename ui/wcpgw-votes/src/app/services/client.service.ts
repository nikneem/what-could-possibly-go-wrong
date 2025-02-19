import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ClientService {
  constructor() {}

  public getClientId(): string {
    return this.getClientIdFromStorage();
  }

  private getClientIdFromStorage(): string {
    const key = 'VotR-Client-Id';
    let clientId = localStorage.getItem(key);
    if (!clientId) {
      clientId = this.generateId();
      localStorage.setItem(key, clientId);
    }
    return clientId;
  }

  private generateId(): string {
    // Return a GUID that complies with RFC4122
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(
      /[xy]/g,
      function (c) {
        // tslint:disable-next-line:no-bitwise
        const r = (Math.random() * 16) | 0;
        // tslint:disable-next-line:no-bitwise
        const v = c === 'x' ? r : (r & 0x3) | 0x8;
        return v.toString(16);
      }
    );
  }
}
