import { Injectable } from "@angular/core";
import { HttpClient, HttpParams } from "@angular/common/http";
import { Observable } from "rxjs";
import { Story, StoryResponse } from "../models/story.model";

@Injectable({
  providedIn: "root",
})
export class StoryService {
  private baseUrl = "http://localhost:5091/api/stories";

  constructor(private http: HttpClient) {}

  getStories(page = 1, size = 20, search?: string): Observable<StoryResponse> {
    let params = new HttpParams()
      .set("page", page.toString())
      .set("size", size.toString());

    if (search) {
      params = params.set("search", search);
    }

    return this.http.get<StoryResponse>(`${this.baseUrl}/newest`, { params });
  }
}
