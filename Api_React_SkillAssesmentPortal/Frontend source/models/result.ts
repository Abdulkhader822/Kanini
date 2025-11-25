export interface ResultCreateDto {
  userId: number;
  testId: number;
  testLevelId: number;
  timeTakenSecs: number;
  answers: import("./question").QuestionAnswer[];
}

export interface ResultResponseDto {
  resultId: number;
  userId: number;
  userName: string;
  testId: number;
  testName: string;
  testLevelId: number;
  score: number;
  percentage: number;      // backend decimal → number
  resultStatus: "Pass" | "Fail";
  suggestion: string;
  dateAttempted: string;
  attemptNumber?: number;
  isReattempt?: boolean;
  hasCertificate?: boolean;
  isFinalLevelCleared?: boolean;
}
