import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { Track } from "../types/Track";
import { useNotifier } from "../../shared/hooks/Notifier.hook";
import TrackService from "../Track.service";
import Loading from "../../shared/components/Loading/Loading";
import NotFound from "../../shared/components/NotFound/NotFound";
import { Avatar, Grid, Paper, Stack, Typography } from "@mui/material";
import FileService from "../../shared/services/File.service";
import TrackInfoEditor from "../components/TrackInfoEditor/TrackInfoEditor";
import TrackDeleter from "../components/TrackDeleter/TrackDeleter";
import TrackFileEditor from "../components/TrackFileEditor/TrackFileEditor";

export default function TrackDetailsPage() {
  const { id } = useParams() as { id: string };
  const [track, setTrack] = useState<Track>();
  const [loading, setLoading] = useState(true);
  const { showError } = useNotifier();

  useEffect(() => {
    TrackService.get(id)
      .then((track) => setTrack(track))
      .catch(showError)
      .then(() => setLoading(false));
  }, [id]);

  if (loading) {
    return <Loading />;
  }

  if (!track) {
    return <NotFound />;
  }

  return (
    <Grid container spacing={2} justifyContent={"center"}>
      <Grid item xs={12} md={10} lg={8}>
        <Paper sx={{ p: 4 }} elevation={10}>
          <Typography variant={"h3"} component={"div"}>
            {track.name}
          </Typography>

          <Grid container spacing={2}>
            <Grid item xs={12} md={6}>
              <Stack spacing={2} alignItems={"center"}>
                <Avatar
                  sx={{ width: 250, height: 250 }}
                  src={
                    track.album
                      ? FileService.getLocation(track.album.coverLocation)
                      : "https://www.dailymaverick.co.za/wp-content/uploads/nvmnd3.jpg"
                  }
                />
                <TrackFileEditor track={track} setTrack={setTrack} />
                <TrackDeleter track={track} setTrack={setTrack} />
              </Stack>
            </Grid>

            <Grid item xs={12} md={6}>
              <TrackInfoEditor track={track} setTrack={setTrack} />
            </Grid>
          </Grid>
        </Paper>
      </Grid>
    </Grid>
  );
}
