import { useState, useEffect } from "react";
import {
  Box,
  Button,
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Stack,
} from "@mui/material";
import { toast } from 'react-toastify';
import { http } from "../../Api/http";

interface Category {
  categoryId: number;
  categoryName: string;
  description: string;
}

export default function CategoryManager() {
  const [categories, setCategories] = useState<Category[]>([]);
  const [form, setForm] = useState({ categoryName: "", description: "" });
  const [editId, setEditId] = useState<number | null>(null);

  const load = async () => {
    const { data } = await http.get<Category[]>("/Category");
    setCategories(data);
  };

  const save = async () => {
    try {
      if (editId) {
        await http.put(`/Category/${editId}`, form);
        toast.success("Category updated successfully", { toastId: "category-updated" });
      } else {
        await http.post("/Category", form);
        toast.success("Category added successfully", { toastId: "category-added" });
      }
      setForm({ categoryName: "", description: "" });
      setEditId(null);
      load();
    } catch (err: any) {
      // Error handled by interceptor
    }
  };

  const remove = async (id: number) => {
    try {
      await http.delete(`/Category/${id}`);
      toast.success("Category deleted successfully", { toastId: "category-deleted" });
      load();
    } catch (err: any) {
      // Error handled by interceptor
    }
  };

  useEffect(() => {
    load();
  }, []);

  return (
    <Box>
      <Typography variant="h5" fontWeight="600" gutterBottom sx={{ color: "#667eea", mb: 3 }}>
        📂 Category Management
      </Typography>

      <Stack direction="row" spacing={2} mb={3}>
        <TextField
          label="Category Name"
          value={form.categoryName}
          onChange={(e) => setForm({ ...form, categoryName: e.target.value })}
          fullWidth
        />
        <TextField
          label="Description"
          value={form.description}
          onChange={(e) => setForm({ ...form, description: e.target.value })}
          fullWidth
        />
        <Button
          variant="contained"
          onClick={save}
          sx={{ 
            whiteSpace: "nowrap",
            background: "linear-gradient(135deg, #667eea, #764ba2)",
            "&:hover": { background: "linear-gradient(135deg, #5a67d8, #6b46c1)" }
          }}
        >
          {editId ? "Update" : "Add"}
        </Button>
        {editId && (
          <Button
            variant="outlined"
            onClick={() => {
              setEditId(null);
              setForm({ categoryName: "", description: "" });
            }}
            sx={{ 
              whiteSpace: "nowrap",
              borderColor: "#667eea",
              color: "#667eea",
              "&:hover": { borderColor: "#5a67d8", color: "#5a67d8" }
            }}
          >
            Cancel
          </Button>
        )}
      </Stack>

      <TableContainer component={Paper} sx={{ borderRadius: 3, border: "1px solid #e0e0e0" }}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell><b>ID</b></TableCell>
              <TableCell><b>Name</b></TableCell>
              <TableCell><b>Description</b></TableCell>
              <TableCell align="right"><b>Actions</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {categories.map((c) => (
              <TableRow key={c.categoryId}>
                <TableCell>{c.categoryId}</TableCell>
                <TableCell>{c.categoryName}</TableCell>
                <TableCell>{c.description}</TableCell>
                <TableCell align="right">
                  <Stack direction="row" spacing={1} justifyContent="flex-end">
                    <Button
                      variant="outlined"
                      size="small"
                      onClick={() => {
                        setEditId(c.categoryId);
                        setForm(c);
                      }}
                      sx={{
                        borderColor: "#667eea",
                        color: "#667eea",
                        "&:hover": { borderColor: "#5a67d8", color: "#5a67d8" }
                      }}
                    >
                      Edit
                    </Button>
                    <Button
                      variant="outlined"
                      color="error"
                      size="small"
                      onClick={() => remove(c.categoryId)}
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