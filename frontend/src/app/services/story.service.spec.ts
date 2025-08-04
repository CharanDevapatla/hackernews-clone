import { TestBed } from "@angular/core/testing";
import {
  HttpClientTestingModule,
  HttpTestingController,
} from "@angular/common/http/testing";
import { StoryService } from "./story.service";
import { StoryResponse } from "../models/story.model";

describe("StoryService", () => {
  let service: StoryService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [StoryService],
    });
    service = TestBed.inject(StoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it("should be created", () => {
    expect(service).toBeTruthy();
  });

  it("should get stories with default parameters", () => {
    const mockResponse: StoryResponse = {
      stories: [],
      totalCount: 0,
      pageNumber: 1,
      pageSize: 20,
    };

    service.getStories().subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(
      "http://localhost:5091/api/stories?pageNumber=1&pageSize=20",
    );
    expect(req.request.method).toBe("GET");
    req.flush(mockResponse);
  });

  it("should get stories with custom parameters", () => {
    const mockResponse: StoryResponse = {
      stories: [],
      totalCount: 0,
      pageNumber: 2,
      pageSize: 10,
    };

    service.getStories(2, 10, "angular").subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(
      "http://localhost:5091/api/stories?pageNumber=2&pageSize=10&search=angular",
    );
    expect(req.request.method).toBe("GET");
    req.flush(mockResponse);
  });

  it("should handle search term with spaces", () => {
    const mockResponse: StoryResponse = {
      stories: [],
      totalCount: 0,
      pageNumber: 1,
      pageSize: 20,
    };

    service.getStories(1, 20, "hello world").subscribe((response) => {
      expect(response).toEqual(mockResponse);
    });

    const req = httpMock.expectOne(
      "http://localhost:5091/api/stories?pageNumber=1&pageSize=20&search=hello%20world",
    );
    expect(req.request.method).toBe("GET");
    req.flush(mockResponse);
  });
});
