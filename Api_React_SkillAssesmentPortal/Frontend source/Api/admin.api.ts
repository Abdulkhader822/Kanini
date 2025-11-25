import { http } from "./http";


export interface UserDto {
  userId?: number;
  name: string;
  email: string;
  password?: string;
  role: string;
}

export async function getUsers() {
  const { data } = await http.get<UserDto[]>("/User");
  return data;
}

export async function addUser(payload: UserDto) {
  try {
    const { data } = await http.post("/User", payload);
    return data;
  } catch (error: any) {
    throw new Error(error.response?.data?.message || "Failed to add user");
  }
}

export async function deleteUser(userId: number) {
  try {
    await http.delete(`/User/${userId}`);
  } catch (error: any) {
    throw new Error(error.response?.data?.message || "Failed to delete user");
  }
}


export interface CategoryDto {
  categoryId?: number;
  categoryName: string;
  description: string;
}

export async function getCategories() {
  const { data } = await http.get<CategoryDto[]>("/Category");
  return data;
}

export async function addCategory(payload: CategoryDto) {
  try {
    const { data } = await http.post("/Category", payload);
    return data;
  } catch (error: any) {
    throw new Error(error.response?.data?.message || "Failed to add category");
  }
}


export interface TestDto {
  testId?: number;
  categoryId: number;
  testName: string;
  totalQuestions: number;
  durationMins: number;
  createdBy: number;
}

export async function getTests() {
  const { data } = await http.get<TestDto[]>("/Test");
  return data;
}

export async function addTest(payload: TestDto) {
  try {
    const { data } = await http.post("/Test", payload);
    return data;
  } catch (error: any) {
    throw new Error(error.response?.data?.message || "Failed to create test");
  }
}

 

export interface TestLevelDto {
  testLevelId?: number;
  testId: number;
  levelName: string;
  passingScore: number;
  videoLink: string;
  durationMins: number;
}

export async function getTestLevels() {
  const { data } = await http.get<TestLevelDto[]>("/TestLevel");
  return data;
}

export async function addTestLevel(payload: TestLevelDto) {
  try {
    const { data } = await http.post("/TestLevel", payload);
    return data;
  } catch (error: any) {
    throw new Error(error.response?.data?.message || "Failed to add test level");
  }
}



export interface QuestionDto {
  questionText: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctOption: string;
}

export interface BulkQuestionRequest {
  testLevelId: number;
  questions: QuestionDto[];
}

export async function getQuestionsByLevel(levelId: number) {
  const { data } = await http.get(`/Question/level/${levelId}`);
  return data;
}

export async function addBulkQuestions(payload: BulkQuestionRequest) {
  try {
    const { data } = await http.post("/Question/bulk", payload);
    return data;
  } catch (error: any) {
    throw new Error(
      error.response?.data?.message ||
      "Failed to add questions (Check level limits or total question limit)"
    );
  }
}



export interface CertificateDto {
  certificateId: number;
  userId: number;
  testId: number;
  issueDate: string;
  certificateURL: string;
}

export async function getCertificates() {
  const { data } = await http.get<CertificateDto[]>("/Certificate");
  return data;
}
