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
      author: 'testuser1',
      publishedDate: '2022-01-01T00:00:00Z',
      score: 100,
      commentsCount: 25
    },
    {
      id: 2,
      title: 'Test Story 2',
      author: 'testuser2',
      publishedDate: '2022-01-01T00:01:40Z',
      score: 50,
      commentsCount: 10
    }
  ];

  const mockResponse: StoryResponse = {
    items: mockStories,
    totalCount: 2,
    pageNumber: 1,
    pageSize: 20,
    totalPages: 1
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

    expect(mockStoryService.getStories).toHaveBeenCalledWith(1, 20, undefined);
    expect(component.stories).toEqual(mockStories);
    expect(component.totalPages).toBe(1);
    expect(component.isLoading).toBeFalse();
  });

  it('should handle error when loading stories', () => {
    mockStoryService.getStories.and.returnValue(throwError(() => new Error('API Error')));

    component.ngOnInit();

    expect(component.errorMsg).toBe('Could not load stories');
    expect(component.isLoading).toBeFalse();
    expect(component.stories).toEqual([]);
  });

  it('should search stories', () => {
    mockStoryService.getStories.and.returnValue(of(mockResponse));
    component.searchQuery = 'test';
    component.pageNumber = 2;

    component.onSearch();

    expect(component.pageNumber).toBe(1);
    expect(mockStoryService.getStories).toHaveBeenCalledWith(1, 20, 'test');
  });

  it('should go to next page', () => {
    mockStoryService.getStories.and.returnValue(of(mockResponse));
    component.totalPages = 5;
    component.pageNumber = 1;
    component.searchQuery = '';

    component.changePage(2);

    expect(component.pageNumber).toBe(2);
    expect(mockStoryService.getStories).toHaveBeenCalledWith(2, 20, '');
  });

  it('should not go to invalid page', () => {
    component.totalPages = 5;
    component.pageNumber = 1;

    component.changePage(0);
    expect(component.pageNumber).toBe(1);

    component.changePage(6);
    expect(component.pageNumber).toBe(1);
  });

  it('should get story URL', () => {
    const storyWithUrl = mockStories[0];
    const storyWithoutUrl: Story = {
      ...mockStories[1],
      url: undefined
    };

    expect(component.getUrl(storyWithUrl)).toBe('https://example.com/1');
    expect(component.getUrl(storyWithoutUrl)).toBe('https://news.ycombinator.com/item?id=2');
  });
});