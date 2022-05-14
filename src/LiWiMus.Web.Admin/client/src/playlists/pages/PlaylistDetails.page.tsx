import React, { useEffect, useState } from "react";
import { Link as RouterLink, useParams } from "react-router-dom";
import { Playlist } from "../types/Playlist";
import Loading from "../../shared/components/Loading/Loading";
import NotFound from "../../shared/components/NotFound/NotFound";
import { Grid, Link, Stack, Typography } from "@mui/material";
import playlistCover from "../images/playlist-cover-negative.png";
import { useSnackbar } from "notistack";
import PlaylistImageEditor from "../components/PlaylistImageEditor/PlaylistImageEditor";
import PlaylistInfoEditor from "../components/PlaylistInfoEditor/PlaylistInfoEditor";
import PlaylistPublicityEditor from "../components/PlaylistPublicityEditor/PlaylistPublicityEditor";
import InfoCard from "../../shared/components/InfoCard/InfoCard";
import PlaylistDeleter from "../components/PlaylistDeleter/PlaylistDeleter";
import PlaylistService from "../Playlist.service";
import ReadonlyInfo from "../../shared/components/InfoItem/ReadonlyInfo";
import PlaylistTracks from "../components/PlaylistTracks/PlaylistTracks";
import { Track } from "../../tracks/types/Track";

const API_URL = process.env.REACT_APP_API_URL;

export default function PlaylistDetailsPage() {
  const { id } = useParams() as { id: string };
  const [playlist, setPlaylist] = useState<Playlist>();
  const [playlistPhoto, setPlaylistPhoto] = useState<string>("");
  const [loading, setLoading] = useState(true);
  const { enqueueSnackbar } = useSnackbar();

  const setPlaylistWithPhoto = (playlist: Playlist) => {
    const photoLocation = playlist.photoLocation
      ? API_URL + playlist.photoLocation
      : playlistCover;
    setPlaylist({ ...playlist });
    setPlaylistPhoto(photoLocation);
  };

  useEffect(() => {
    PlaylistService.get(id)
      .then((playlist) => setPlaylistWithPhoto(playlist))
      .catch((error) => enqueueSnackbar(error.message, { variant: "error" }))
      .then(() => setLoading(false));
  }, [id]);

  useEffect(() => {});

  if (loading) {
    return <Loading />;
  }

  if (!playlist) {
    return <NotFound />;
  }

  return (
    <>
      <h1>{playlist.name}</h1>

      <Grid
        container
        spacing={10}
        justifyContent={"space-around"}
        sx={{ mb: 10 }}
      >
        <Grid
          item
          xs={12}
          md={8}
          lg={4}
          sx={{
            alignItems: "center",
            display: "flex",
            flexDirection: "column",
          }}
        >
          <PlaylistImageEditor
            playlist={playlist}
            playlistPhoto={playlistPhoto}
            setPlaylistWithPhoto={setPlaylistWithPhoto}
          />
        </Grid>
        <Grid item xs={12} md={8} lg={4}>
          <Stack direction={"column"} spacing={3} alignItems={"end"}>
            <ReadonlyInfo name={"ID"} value={playlist.id} />
            <ReadonlyInfo
              name={"User"}
              value={
                <Link
                  component={RouterLink}
                  to={`/admin/users/${playlist.userId}`}
                  underline="none"
                  color={"secondary"}
                >
                  {playlist.userName}
                </Link>
              }
            />
            <PlaylistInfoEditor
              id={id}
              dto={{ name: playlist.name }}
              setDto={(dto) => {
                setPlaylist({ ...playlist, ...dto });
              }}
            />
            <PlaylistPublicityEditor
              id={id}
              dto={{ isPublic: playlist.isPublic }}
              setDto={(dto) => {
                setPlaylist({ ...playlist, ...dto });
              }}
            />
            <PlaylistDeleter playlist={playlist} setPlaylist={setPlaylist} />
          </Stack>
        </Grid>
        <Grid item xs={12} md={8} lg={4}>
          <Stack spacing={4}>
            <InfoCard title={"Tracks"} value={playlist.tracksCount} />
            <InfoCard title={"Listeners"} value={playlist.listenersCount} />
          </Stack>
        </Grid>
      </Grid>
    </>
  );
}
