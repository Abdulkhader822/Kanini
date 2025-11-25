import { useEffect, useState } from "react";
import {
  Box,
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Select,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TextField,
  Typography,
  Paper,
} from "@mui/material";
import { toast } from 'react-toastify';
import { http } from "../../Api/http";

interface Test {
  testId: number;
  testName: string;
}
interface TestLevel {
  testLevelId: number;
  testId: number;
  levelName: string;
  passingScore: number;
  videoLink: string;
  durationMins: number;
}

export default function TestLevelManager() {
  const [tests, setTests] = useState<Test[]>([]);
  const [levels, setLevels] = useState<TestLevel[]>([]);
  const [form, setForm] = useState({
    testId: 0,
    levelName: "Easy",
    passingScore: 60,
    videoLink: "",
    durationMins: 30,
  });
  const [editId, setEditId] = useState<number | null>(null);

  const loadTests = async () => {
    const { data } = await http.get<Test[]>("/Test");
    setTests(data);
  };

  const loadLevels = async (testId?: number) => {
    if (!testId) return;
    const { data } = await http.get<TestLevel[]>(`/TestLevel/test/${testId}`);
    setLevels(data);
  };

  const validateYouTubeLink = (url: string) => {
    if (!url) return true;
    const youtubeRegex = /^(https?\:\/\/)?(www\.)?(youtube\.com|youtu\.be)\/.+/;
    return youtubeRegex.test(url);
  };

  const save = async () => {
    try {

      
      if (!form.testId) {

        return toast.error("Select a test first!", { toastId: "test-required" });
      }
      if (!form.passingScore || form.passingScore <= 0) return toast.error("Passing score is required and must be greater than 0!", { toastId: "score-required" });
      if (!form.durationMins || form.durationMins <= 0) return toast.error("Duration is required and must be greater than 0!", { toastId: "duration-required" });
      if (!form.videoLink) return toast.error("YouTube link is required!", { toastId: "video-required" });
      if (!validateYouTubeLink(form.videoLink)) {
        return toast.error("Please enter a valid YouTube URL", { toastId: "video-invalid" });
      }
      
      const currentTestId = form.testId;
      
      if (editId) {

        // Backend doesn't support PUT, so we'll delete and recreate
        await http.delete(`/TestLevel/${editId}`);
        await http.post("/TestLevel", form);
        toast.success("Level updated successfully", { toastId: "level-updated" });
      } else {

        await http.post("/TestLevel", form);
        toast.success("Level added successfully", { toastId: "level-added" });
      }
      setForm({ testId: currentTestId, levelName: "Easy", passingScore: 60, videoLink: "", durationMins: 30 });
      setEditId(null);
      loadLevels(currentTestId);
    } catch (err: any) {
      console.error("Save error:", err);
      // Error handled by interceptor
    }
  };

  const remove = async (id: number) => {
    try {
      await http.delete(`/TestLevel/${id}`);
      toast.success("Level deleted successfully", { toastId: "level-deleted" });
      loadLevels(form.testId);
    } catch (err: any) {
      // Error handled by interceptor
    }
  };

  useEffect(() => {
    loadTests();
  }, []);

  return (
    <Box sx={{ maxWidth: 900, mx: "auto", p: 3, background: "#fff", borderRadius: 3, boxShadow: 3 }}>
      <Typography variant="h6" fontWeight="bold" gutterBottom>
        📊 Test Level Management
      </Typography>

      <Stack direction={{ xs: "column", md: "row" }} spacing={2} mb={3}>
        <FormControl fullWidth>
          <InputLabel>Select Test</InputLabel>
          <Select
            value={form.testId || ""}
            onChange={(e) => {
              const id = Number(e.target.value);
              setForm({ ...form, testId: id });
              loadLevels(id);
            }}
            label="Select Test"
          >
            {tests.map((t) => (
              <MenuItem key={t.testId} value={t.testId}>
                {t.testName}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <FormControl fullWidth>
          <InputLabel>Level</InputLabel>
          <Select
            value={form.levelName}
            onChange={(e) => setForm({ ...form, levelName: e.target.value })}
            label="Level"
          >
            {["Easy", "Medium", "Hard"].map((level) => (
              <MenuItem key={level} value={level}>
                {level}
              </MenuItem>
            ))}
          </Select>
        </FormControl>

        <TextField
          type="number"
          label="Passing Score"
          value={form.passingScore}
          onChange={(e) => setForm({ ...form, passingScore: Number(e.target.value) })}
          required
        />
        <TextField
          label="YouTube Link"
          value={form.videoLink}
          onChange={(e) => setForm({ ...form, videoLink: e.target.value })}
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
              setForm({ testId: form.testId, levelName: "Easy", passingScore: 60, videoLink: "", durationMins: 30 });
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
              <TableCell><b>Level</b></TableCell>
              <TableCell><b>Passing Score</b></TableCell>
              <TableCell><b>Duration</b></TableCell>
              <TableCell align="right"><b>Actions</b></TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {levels.map((l) => (
              <TableRow key={l.testLevelId}>
                <TableCell>{l.testLevelId}</TableCell>
                <TableCell>{l.levelName}</TableCell>
                <TableCell>{l.passingScore}</TableCell>
                <TableCell>{l.durationMins}</TableCell>
                <TableCell align="right">
                  <Stack direction="row" spacing={1} justifyContent="flex-end">
                    <Button
                      variant="outlined"
                      color="primary"
                      size="small"
                      onClick={() => {

                        setEditId(l.testLevelId);
                        setForm({
                          testId: l.testId,
                          levelName: l.levelName,
                          passingScore: l.passingScore,
                          videoLink: l.videoLink,
                          durationMins: l.durationMins
                        });
                      }}
                    >
                      Edit
                    </Button>
                    <Button
                      variant="outlined"
                      color="error"
                      size="small"
                      onClick={() => remove(l.testLevelId)}
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