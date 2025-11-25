export interface QuestionResponse {
  questionId: number;
  testLevelId: number;
  questionText: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
}

export interface QuestionCreate {
  testLevelId: number;
  questionText: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctOption: "A" | "B" | "C" | "D";
}

export interface BulkQuestionCreate {
  testLevelId: number;
  questions: QuestionCreate[];
}

export interface QuestionAnswer {
  questionId: number;
  selectedOption: "A" | "B" | "C" | "D";
}
