PGDMP     
    ,         	        z         	   flightsdb    14.2    14.2 
    �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                      false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                      false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                      false            �           1262    16415 	   flightsdb    DATABASE     f   CREATE DATABASE flightsdb WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE = 'Russian_Russia.1251';
    DROP DATABASE flightsdb;
                postgres    false            �            1259    16417    segments    TABLE     �  CREATE TABLE public.segments (
    id_segment integer NOT NULL,
    airline_code character varying(500),
    flight_num integer,
    depart_place character varying(500),
    depart_datetime timestamp with time zone,
    arrive_place character varying(500),
    arrive_datetime timestamp with time zone,
    pnr_id character varying(500),
    ticket_number character varying(500),
    passed boolean
);
    DROP TABLE public.segments;
       public         heap    postgres    false            �            1259    16416    segments_id_segment_seq    SEQUENCE     �   CREATE SEQUENCE public.segments_id_segment_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;
 .   DROP SEQUENCE public.segments_id_segment_seq;
       public          postgres    false    210            �           0    0    segments_id_segment_seq    SEQUENCE OWNED BY     S   ALTER SEQUENCE public.segments_id_segment_seq OWNED BY public.segments.id_segment;
          public          postgres    false    209            \           2604    16420    segments id_segment    DEFAULT     z   ALTER TABLE ONLY public.segments ALTER COLUMN id_segment SET DEFAULT nextval('public.segments_id_segment_seq'::regclass);
 B   ALTER TABLE public.segments ALTER COLUMN id_segment DROP DEFAULT;
       public          postgres    false    210    209    210            �          0    16417    segments 
   TABLE DATA           �   COPY public.segments (id_segment, airline_code, flight_num, depart_place, depart_datetime, arrive_place, arrive_datetime, pnr_id, ticket_number, passed) FROM stdin;
    public          postgres    false    210          �           0    0    segments_id_segment_seq    SEQUENCE SET     F   SELECT pg_catalog.setval('public.segments_id_segment_seq', 13, true);
          public          postgres    false    209            �   =  x����N�0Eד������EjDKP�<T�b����n��
����L<4@�u7 #s�R �h<�G�C���M��$)��SUe��J--�{�0��������9��m7����%q,Vˈ-��9'�ܽL]*(�V������E�)��R����B7)�Y��k쒛��a�"��K���􄭠X�u�:u�4[���l��:"�߱�I�n��9��f���!�����^G_Vv�~d�n`ի��J�n`�.\|3N]Sŗ^��M�"�Y������Ē3�8�V[ڒ�+��EcIl����{�}�ϲ��O�     