﻿import {IdDto} from "../../shared/dto/id.dto";
import {Column, JoinColumn, ManyToOne, OneToMany} from "typeorm";
import {User} from "../../users/user.entity";
import {PlaylistTrack} from "../../playlistTracks/playlistTrack.entity";
import {Exclude, Expose, Type} from "class-transformer";
import {UserDto} from "../../users/dto/user.dto";
import {IsBoolean, IsNotEmpty, IsString, Length, MaxLength, ValidateNested} from "class-validator";
import {ApiProperty} from "@nestjs/swagger";

@Exclude()
export class PlaylistDto extends IdDto {
    @ApiProperty()
    @Expose()
    owner: UserDto;

    @ApiProperty()
    @Expose()
    name: string;

    @ApiProperty()
    @Expose()
    isPublic: boolean;

    @ApiProperty()
    @Expose()
    @IsString()
    photoLocation: string;

    @ApiProperty()
    @Expose()
    createdAt: Date;

    @ApiProperty()
    @Expose()
    modifiedAt: Date;
}