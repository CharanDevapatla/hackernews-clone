export interface Story {
  id: number;
  title: string;
  url?: string;
  author: string;
  publishedDate: string;
  score: number;
  commentsCount: number;
}

export interface StoryResponse {
  items: Story[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}