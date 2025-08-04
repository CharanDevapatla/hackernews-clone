import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { StoryList } from './story-list';
import { StoryService } from '../services/story.service';
import { Story, StoryResponse } from '../models/story.model';

describe('StoryList', () => {
  let component: StoryList;
  let fixture: ComponentFixture<StoryList>;
  let mockStoryService: jasmine.SpyObj<StoryService>;

  const mockStories: Story[] = [
    {
      id: 1,
      title: 'Test Story 1',
      url: 'https://example.com/1',
      by: 'testuser1',
      time: 1640995200,
      score: 100,
      type: 'story'
    },
    {
      id: 2,
      title: 'Test Story 2',
      by: 'testuser2',
      time: 1640995300,
      score: 50,
      type: 'story'
    }
  ];

  const mockResponse: StoryResponse = {
    stories: mockStories,
    totalCount: 2,
    pageNumber: 1,
    pageSize: 20
  };

  beforeEach(async () => {
    const spy = jasmine.createSpyObj('StoryService', ['getStories']);

    await TestBed.configureTestingModule({
      imports: [StoryList],
      providers: [
        { provide: StoryService, useValue: spy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StoryList);
    component = fixture.componentInstance;
    mockStoryService = TestBed.inject(StoryService) as jasmine.SpyObj<StoryService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load stories on init', () => {
    mockStoryService.getStories.and.returnValue(of(mockResponse));

    component.ngOnInit();

    expect(mockStoryService.getStories).toHaveBeenCalledWith(1, 20, '');
    expect(component.stories).toEqual(mockStories);
    expect(component.totalPages).toBe(1);
    expect(component.loading).toBeFalse();
  });

  it('should handle error when loading stories', () => {
    mockStoryService.getStories.and.returnValue(throwError(() => new Error('API Error')));

    component.ngOnInit();

    expect(component.error).toBe('Failed to load stories. Please try again.');
    expect(component.loading).toBeFalse();
    expect(component.stories).toEqual([]);
  });

  it('should search stories', () => {
    mockStoryService.getStories.and.returnValue(of(mockResponse));
    component.searchTerm = 'test';
    component.currentPage = 2;

    component.search();

    expect(component.currentPage).toBe(1);
    expect(mockStoryService.getStories).toHaveBeenCalledWith(1, 20, 'test');
  });

  it('should go to next page', () => {
    mockStoryService.getStories.and.returnValue(of(mockResponse));
    component.totalPages = 5;
    component.currentPage = 1;

    component.goToPage(2);

    expect(component.currentPage).toBe(2);
    expect(mockStoryService.getStories).toHaveBeenCalledWith(2, 20, '');
  });

  it('should not go to invalid page', () => {
    component.totalPages = 5;
    component.currentPage = 1;

    component.goToPage(0);
    expect(component.currentPage).toBe(1);

    component.goToPage(6);
    expect(component.currentPage).toBe(1);
  });

  it('should get story URL', () => {
    const storyWithUrl = mockStories[0];
    const storyWithoutUrl = mockStories[1];

    expect(component.getStoryUrl(storyWithUrl)).toBe('https://example.com/1');
    expect(component.getStoryUrl(storyWithoutUrl)).toBe('https://news.ycombinator.com/item?id=2');
  });
});