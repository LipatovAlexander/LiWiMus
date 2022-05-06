import { Module } from '@nestjs/common';
import { AppController } from './app.controller';
import { AppService } from './app.service';
import { TypeOrmModule } from '@nestjs/typeorm';
import { ServeStaticModule } from '@nestjs/serve-static';
import { join } from 'path';
import {UsersModule} from "./users/users.module";
import { ArtistsController } from './artists/artists.controller';
import { ArtistsModule } from './artists/artists.module';
import { TracksModule } from './tracks/tracks.module';

@Module({
  imports: [
    TypeOrmModule.forRoot(),
    ServeStaticModule.forRoot({
      rootPath: join(__dirname, '..', 'public'),
      serveRoot: '/admin',
    }),
    UsersModule,
    ArtistsModule,
    TracksModule
  ],
  controllers: [AppController],
  providers: [AppService],
})
export class AppModule {}
