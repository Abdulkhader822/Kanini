export interface TestResponse {
  testId: number;
  categoryId: number;
  testName: string;
  totalQuestions: number;  // e.g., 30
  durationMins: number;    // e.g., 180
  totalMarks: number;      // typically 100
  createdBy: number;
  createdAt: string;

  // server-enriched (optional)
  categoryName?: string;
  createdByName?: string;
  marksPerQuestion?: number;
}

export interface TestCreate {
  categoryId: number;
  testName: string;
  totalQuestions: number;  // divisible by 3
  durationMins: number;    // split by 3 across levels
  createdBy: number;
}

export type TestUpdate = Partial<TestCreate>;
