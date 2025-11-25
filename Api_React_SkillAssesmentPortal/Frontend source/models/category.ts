export interface CategoryResponse {
  categoryId: number;
  categoryName: string;
  description?: string;
}

export interface CategoryCreate {
  categoryName: string;
  description?: string;
}

export type CategoryUpdate = CategoryCreate;
