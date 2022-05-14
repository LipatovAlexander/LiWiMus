import React from "react";
import { Button } from "@mui/material";
import { useSnackbar } from "notistack";
import PlaylistService from "../../Playlist.service";

type Inputs = {
  isPublic: boolean;
};

type Props = {
  id: string;
  dto: Inputs;
  setDto: (dto: Inputs) => void;
};

export default function PlaylistPublicityEditor({ id, dto, setDto }: Props) {
  const { enqueueSnackbar } = useSnackbar();

  const publicityHandler = async () => {
    const req = { id, isPublic: !dto.isPublic };
    try {
      const data = (await PlaylistService.update(req)) as Inputs;
      setDto(data);
      enqueueSnackbar(`Playlist is ${data.isPublic ? "public" : "private"}`, {
        variant: "success",
      });
    } catch (error) {
      // @ts-ignore
      enqueueSnackbar(error.message, { variant: "error" });
    }
  };

  return (
    <Button
      variant="contained"
      color={"error"}
      sx={{ borderRadius: "20px", px: 4 }}
      onClick={publicityHandler}
    >
      Make {dto.isPublic ? "private" : "public"}
    </Button>
  );
}
