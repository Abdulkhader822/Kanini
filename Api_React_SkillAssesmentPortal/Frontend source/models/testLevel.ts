export type LevelName = "Easy" | "Medium" | "Hard";

export interface TestLevelResponse {
  testLevelId: number;
  testId: number;
  levelName: LevelName;
  passingScore: number;    // %
  videoLink?: string;
  durationMins: number;    // per-level duration
}

export interface TestLevelCreate extends Omit<TestLevelResponse, "testLevelId"> {}
export type TestLevelUpdate = Partial<TestLevelCreate>;
