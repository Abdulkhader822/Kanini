import { useEffect, useState } from "react";
import {
  Box,
  Button,
  TextField,
  Select,
  MenuItem,
  FormControl,
  InputLabel,
  Typography,
  Paper,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
} from "@mui/material";
import { toast } from 'react-toastify';
import { http } from "../../Api/http";

interface Category {
  categoryId: number;
  categoryName: string;
}
interface Test {
  testId: number;
  testName: string;
  totalQuestions: number;
  durationMins: number;
  categoryId: number;
}

export default function TestManager() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [tests, setTests] = useState<Test[]>([]);
  const [form, setForm] = useState({
    testName: "",
    totalQuestions: 30,
    durationMins: 180,
    categoryId: 0,
  });
  const [selectedCategory, setSelectedCategory] = useState<number | "">("");
  const [editId, setEditId] = useState<number | null>(null);

  const loadCategories = async () => {
    const { data } = await http.get<Category[]>("/Category");
    setCategories(data);
  };

  const loadTests = async (categoryId?: number) => {
    if (!categoryId) return;
    const { data } = await http.get<Test[]>(`/Test/category/${categoryId}`);
    setTests(data);
  };

  const save = async () => {
    try {

      
      if (!form.categoryId) return toast.error("Select a category first!", { toastId: "category-required" });
      if (!form.testName.trim()) return toast.error("Test name is required!", { toastId: "testname-required" });
      if (!form.totalQuestions || form.totalQuestions <= 0) return toast.error("Total questions must be greater than 0!", { toastId: "questions-required" });
      if (!form.durationMins || form.durationMins <= 0) return toast.error("Duration must be greater than 0!", { toastId: "duration-required" });
      
      const currentCategoryId = form.categoryId;
      
      if (editId) {
        await http.put(`/Test/${editId}`, form);
        toast.success("Test updated successfully", { toastId: "test-updated" });
      } else {
        await http.post("/Test", { ...form, createdBy: 1 });
        toast.success("Test added successfully", { toastId: "test-added" });
      }
      setForm({ testName: "", totalQuestions: 30, durationMins: 180, categoryId: currentCategoryId });
      setEditId(null);
      loadTests(currentCategoryId);
    } catch (err: any) {

      // Error handled by interceptor
    }
  };

  const remove = async (id: number) => {
    try {
      await http.delete(`/Test/${id}`);
      toast.success("Test deleted successfully", { toastId: "test-deleted" });
      loadTests(Number(selectedCategory));
    } catch (err: any) {
      // Error handled by interceptor
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  useEffect(() => {
    if (selectedCategory) loadTests(Number(selectedCategory));
  }, [selectedCategory]);

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", p: 3, background: "#fff", borderRadius: 3, boxShadow: 3 }}>
      <Typography variant="h6" fontWeight="bold" gutterBottom>
        🧩 Test Management
      </Typography>

      <Stack direction={{ xs: "column", md: "row" }} spacing={2} mb={3}>
        <FormControl fullWidth>
          <InputLabel>Category</InputLabel>
          <Select
            value={selectedCategory}
            onChange={(e) => {
              const id = Number(e.target.value);
              setSelectedCategory(id);
              setForm({ ...form, categoryId: id });
            }}
            label="Category"
          >
            {categories.map((c) => (
              <MenuItem key={c.categoryId} value={c.categoryId}>
                {c.categoryName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <TextField
          fullWidth
          label="Test Name"
          value={form.testName}
          onChange={(e) => setForm({ ...form, testName: e.target.value })}
          required
        />
        <TextField
          type="number"
          label="Total Questions"
          value={form.totalQuestions}
          onChange={(e) => setForm({ ...form, totalQuestions: Number(e.target.value) })}
          required
        />
        <TextField
          type="number"
          label="Duration (mins)"
          value={form.durationMins}
          onChange={(e) => setForm({ ...form, durationMins: Number(e.target.value) })}
          required
        />
        <Button variant="contained" color="primary" onClick={save}>
          {editId ? "Update" : "Add"}
        </Button>
        {editId && (
          <Button
            variant="outlined"
            onClick={() => {

              setEditId(null);
              setForm({ testName: "", totalQuestions: 30, durationMins: 180, categoryId: Number(selectedCategory) || 0 });
            }}
          >
            Cancel
          </Button>
        )}
      </Stack>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell><b>ID</b></TableCell>
              <TableCell><b>Name</b></TableCell>
              <TableCell><b>Questions</b></TableCell>
              <TableCell><b>Duration</b></TableCell>
              <TableCell align="right"><b>Actions</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {tests.map((t) => (
              <TableRow key={t.testId}>
                <TableCell>{t.testId}</TableCell>
                <TableCell>{t.testName}</TableCell>
                <TableCell>{t.totalQuestions}</TableCell>
                <TableCell>{t.durationMins}</TableCell>
                <TableCell align="right">
                  <Stack direction="row" spacing={1} justifyContent="flex-end">
                    <Button
                      variant="outlined"
                      color="primary"
                      size="small"
                      onClick={() => {

                        setEditId(t.testId);
                        setForm({
                          testName: t.testName,
                          totalQuestions: t.totalQuestions,
                          durationMins: t.durationMins,
                          categoryId: Number(selectedCategory)
                        });
                      }}
                    >
                      Edit
                    </Button>
                    <Button
                      variant="outlined"
                      color="error"
                      size="small"
                      onClick={() => remove(t.testId)}
                    >
                      Delete
                    </Button>
                  </Stack>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </Box>
  );
}