import React from "react";
import { Box, CircularProgress } from "@mui/material";

export default function Loading() {
  return (
    <Box sx={{ display: "flex", justifyContent: "center" }}>
      <CircularProgress size={60} />
    </Box>
  );
}
