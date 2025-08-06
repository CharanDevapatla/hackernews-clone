import { Injectable, inject } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { StoryResponse } from "../models/story.model";
import { environment } from "../../environments/environment";

@Injectable({
  providedIn: "root",
})
export class StoryService {
  private baseUrl = `${environment.apiUrl}/api/stories`;
  private http = inject(HttpClient);

  getStories(pageNumber = 1, pageSize = 20, search?: string): Observable<StoryResponse> {
    let params = new HttpParams()
      .set("pageNumber", pageNumber.toString())
      .set("pageSize", pageSize.toString());

    if (search) {
      params = params.set("search", search);
    }

    return this.http.get<StoryResponse>(`${this.baseUrl}/newest`, { params });
  }
}
