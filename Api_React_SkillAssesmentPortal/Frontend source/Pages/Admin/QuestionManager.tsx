import { useEffect, useState } from "react";
import {
  Box, Button, TextField, Typography, MenuItem, Select, FormControl, InputLabel,
  Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Divider
} from "@mui/material";
import { toast } from 'react-toastify';
import { http } from "../../Api/http";
import type { QuestionResponse } from "../../models/question";

interface Category {
  categoryId: number;
  categoryName: string;
}
interface Test {
  testId: number;
  testName: string;
}
interface Level {
  testLevelId: number;
  levelName: string;
  testName: string;
}
interface Question {
  questionText: string;
  optionA: string;
  optionB: string;
  optionC: string;
  optionD: string;
  correctOption: string;
}

export default function QuestionManager() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [tests, setTests] = useState<Test[]>([]);
  const [levels, setLevels] = useState<Level[]>([]);
  const [existingQuestions, setExistingQuestions] = useState<QuestionResponse[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<number | "">("");
  const [selectedTest, setSelectedTest] = useState<number | "">("");
  const [selectedLevel, setSelectedLevel] = useState<number | "">("");
  const [questions, setQuestions] = useState<Question[]>([
    { questionText: "", optionA: "", optionB: "", optionC: "", optionD: "", correctOption: "A" },
  ]);
  const [editingQuestion, setEditingQuestion] = useState<QuestionResponse | null>(null);
  const [editForm, setEditForm] = useState<Question>({
    questionText: "", optionA: "", optionB: "", optionC: "", optionD: "", correctOption: "A"
  });
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    http.get<Category[]>("/Category").then((res) => setCategories(res.data));
  }, []);

  useEffect(() => {
    if (selectedCategory) {
      http.get<Test[]>(`/Test/category/${selectedCategory}`).then((res) => setTests(res.data));
      setSelectedTest("");
      setLevels([]);
      setExistingQuestions([]);
    } else {
      setTests([]);
    }
  }, [selectedCategory]);

  useEffect(() => {
    if (selectedTest) {
      http.get<Level[]>(`/TestLevel/test/${selectedTest}`).then((res) => setLevels(res.data));
      setSelectedLevel("");
      setExistingQuestions([]);
    } else {
      setLevels([]);
    }
  }, [selectedTest]);

  useEffect(() => {
    if (selectedLevel) {
      loadExistingQuestions();
    } else {
      setExistingQuestions([]);
    }
  }, [selectedLevel]);

  const loadExistingQuestions = async () => {
    try {
      const { data } = await http.get<QuestionResponse[]>(`/Question/testlevel/${selectedLevel}`);
      setExistingQuestions(data);
    } catch (err) {
      console.error("Failed to load questions:", err);
    }
  };

  const addQuestion = () => {
    setQuestions((prev) => [
      ...prev,
      { questionText: "", optionA: "", optionB: "", optionC: "", optionD: "", correctOption: "A" },
    ]);
  };

  const removeQuestion = (index: number) => {
    setQuestions((prev) => prev.filter((_, i) => i !== index));
  };

  const handleChange = (i: number, field: keyof Question, val: string) => {
    setQuestions((prev) => {
      const updated = [...prev];
      updated[i][field] = val;
      return updated;
    });
  };

  const handleSubmit = async () => {
    if (!selectedCategory || !selectedTest || !selectedLevel) {
      toast.error("Select Category, Test and Level first.", { toastId: "selection-required" });
      return;
    }

    try {
      setSaving(true);
      await http.post("/Question/bulk", {
        testId: selectedTest,
        testLevelId: selectedLevel,
        questions,
      });
      toast.success("Questions added successfully!", { toastId: "questions-added" });
      setQuestions([{ questionText: "", optionA: "", optionB: "", optionC: "", optionD: "", correctOption: "A" }]);
      loadExistingQuestions();
    } catch (err: any) {
      // Error handled by interceptor
    } finally {
      setSaving(false);
    }
  };

  const handleEditQuestion = (question: QuestionResponse) => {
    setEditingQuestion(question);
    setEditForm({
      questionText: question.questionText,
      optionA: question.optionA,
      optionB: question.optionB,
      optionC: question.optionC,
      optionD: question.optionD,
      correctOption: "A" // Default since we don't get correct option in response
    });
  };

  const handleUpdateQuestion = async () => {
    if (!editingQuestion) return;
    
    try {
      await http.put(`/Question/${editingQuestion.questionId}`, {
        ...editForm,
        testLevelId: editingQuestion.testLevelId
      });
      toast.success("Question updated successfully!", { toastId: "question-updated" });
      setEditingQuestion(null);
      loadExistingQuestions();
    } catch (err: any) {
      // Error handled by interceptor
    }
  };

  const handleDeleteQuestion = async (questionId: number) => {
    try {
      await http.delete(`/Question/${questionId}`);
      toast.success("Question deleted successfully!", { toastId: "question-deleted" });
      loadExistingQuestions();
    } catch (err: any) {
      // Error handled by interceptor
    }
  };

  return (
    <Box sx={{ maxWidth: 1200, mx: "auto", p: 3, background: "#fff", borderRadius: 3, boxShadow: 3 }}>
      <Typography variant="h6" fontWeight="bold" gutterBottom>
        🧠 Question Management
      </Typography>

      {/* Dropdowns */}
      <Stack direction={{ xs: "column", md: "row" }} spacing={2} mb={3}>
        <FormControl fullWidth>
          <InputLabel>Category</InputLabel>
          <Select
            value={selectedCategory}
            onChange={(e) => setSelectedCategory(Number(e.target.value))}
            label="Category"
          >
            {categories.map((c) => (
              <MenuItem key={c.categoryId} value={c.categoryId}>
                {c.categoryName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControl fullWidth disabled={!selectedCategory}>
          <InputLabel>Test</InputLabel>
          <Select
            value={selectedTest}
            onChange={(e) => setSelectedTest(Number(e.target.value))}
            label="Test"
          >
            {tests.map((t) => (
              <MenuItem key={t.testId} value={t.testId}>
                {t.testName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControl fullWidth disabled={!selectedTest}>
          <InputLabel>Level</InputLabel>
          <Select
            value={selectedLevel}
            onChange={(e) => setSelectedLevel(Number(e.target.value))}
            label="Level"
          >
            {levels.map((l) => (
              <MenuItem key={l.testLevelId} value={l.testLevelId}>
                {l.levelName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>
      </Stack>

      {/* Existing Questions */}
      {selectedLevel && existingQuestions.length > 0 && (
        <>
          <Typography variant="h6" gutterBottom>
            📋 Existing Questions ({existingQuestions.length})
          </Typography>
          <TableContainer component={Paper} sx={{ mb: 3, borderRadius: 2 }}>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell><b>ID</b></TableCell>
                  <TableCell><b>Question</b></TableCell>
                  <TableCell><b>Options</b></TableCell>
                  <TableCell align="right"><b>Actions</b></TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {existingQuestions.map((q) => (
                  <TableRow key={q.questionId}>
                    <TableCell>{q.questionId}</TableCell>
                    <TableCell sx={{ maxWidth: 300 }}>{q.questionText}</TableCell>
                    <TableCell sx={{ maxWidth: 200 }}>
                      A: {q.optionA}<br/>
                      B: {q.optionB}<br/>
                      C: {q.optionC}<br/>
                      D: {q.optionD}
                    </TableCell>
                    <TableCell align="right">
                      <Stack direction="row" spacing={1} justifyContent="flex-end">
                        <Button
                          size="small"
                          variant="outlined"
                          onClick={() => handleEditQuestion(q)}
                        >
                          ✏️ Edit
                        </Button>
                        <Button
                          size="small"
                          variant="outlined"
                          color="error"
                          onClick={() => handleDeleteQuestion(q.questionId)}
                        >
                          🗑️ Delete
                        </Button>
                      </Stack>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
          <Divider sx={{ my: 3 }} />
        </>
      )}

      {/* Edit Question Form */}
      {editingQuestion && (
        <Paper sx={{ p: 3, mb: 3, borderRadius: 2, backgroundColor: "#e3f2fd" }}>
          <Typography variant="h6" gutterBottom>
            ✏️ Edit Question #{editingQuestion.questionId}
          </Typography>
          
          <TextField
            fullWidth
            label="Question Text"
            value={editForm.questionText}
            onChange={(e) => setEditForm({ ...editForm, questionText: e.target.value })}
            sx={{ mb: 2 }}
          />

          <Stack direction={{ xs: "column", sm: "row" }} spacing={2} sx={{ mb: 2 }}>
            {["optionA", "optionB", "optionC", "optionD"].map((opt) => (
              <TextField
                key={opt}
                fullWidth
                label={opt.replace("option", "Option ")}
                value={editForm[opt as keyof Question]}
                onChange={(e) => setEditForm({ ...editForm, [opt]: e.target.value })}
              />
            ))}
          </Stack>

          <FormControl sx={{ width: 160, mr: 2 }}>
            <InputLabel>Correct Option</InputLabel>
            <Select
              value={editForm.correctOption}
              onChange={(e) => setEditForm({ ...editForm, correctOption: e.target.value })}
              label="Correct Option"
            >
              {["A", "B", "C", "D"].map((opt) => (
                <MenuItem key={opt} value={opt}>
                  {opt}
                </MenuItem>
              ))}
            </Select>
          </FormControl>

          <Stack direction="row" spacing={2} sx={{ mt: 2 }}>
            <Button variant="contained" onClick={handleUpdateQuestion}>
              ✅ Update Question
            </Button>
            <Button variant="outlined" onClick={() => setEditingQuestion(null)}>
              Cancel
            </Button>
          </Stack>
        </Paper>
      )}

      {/* Add New Questions */}
      <Typography variant="h6" gutterBottom>
        ➕ Add New Questions
      </Typography>

      {questions.map((q, i) => (
        <Paper key={i} sx={{ p: 3, mb: 2, borderRadius: 2, backgroundColor: "#fafafa" }}>
          <Stack direction="row" justifyContent="space-between" alignItems="center" mb={2}>
            <Typography fontWeight="bold">Question {i + 1}</Typography>
            {questions.length > 1 && (
              <Button color="error" onClick={() => removeQuestion(i)}>
                Remove
              </Button>
            )}
          </Stack>

          <TextField
            fullWidth
            label="Question Text"
            value={q.questionText}
            onChange={(e) => handleChange(i, "questionText", e.target.value)}
            sx={{ mb: 2 }}
          />

          <Stack direction={{ xs: "column", sm: "row" }} spacing={2} sx={{ mb: 2 }}>
            {["optionA", "optionB", "optionC", "optionD"].map((opt) => (
              <TextField
                key={opt}
                fullWidth
                label={opt.replace("option", "Option ")}
                value={q[opt as keyof Question]}
                onChange={(e) => handleChange(i, opt as keyof Question, e.target.value)}
              />
            ))}
          </Stack>

          <FormControl sx={{ width: 160 }}>
            <InputLabel>Correct Option</InputLabel>
            <Select
              value={q.correctOption}
              onChange={(e) => handleChange(i, "correctOption", e.target.value)}
              label="Correct Option"
            >
              {["A", "B", "C", "D"].map((opt) => (
                <MenuItem key={opt} value={opt}>
                  {opt}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
        </Paper>
      ))}

      <Stack direction="row" justifyContent="space-between" alignItems="center" mt={3}>
        <Button variant="outlined" onClick={addQuestion}>
          ➕ Add Question
        </Button>
        <Button
          variant="contained"
          color="success"
          onClick={handleSubmit}
          disabled={saving}
        >
          {saving ? "Saving..." : "✅ Submit All"}
        </Button>
      </Stack>
    </Box>
  );
}