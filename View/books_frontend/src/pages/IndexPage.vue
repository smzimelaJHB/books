<template lang="pug">
q-page
  q-table(:rows='books' :columns='cols' row-key='name' :filter='filter' :filter-options='filterOptions' :loading='loading')
    template(v-slot:top-right='')
      q-btn(color='primary' label='Create Book' @click="image_dialog = true, action='create'")

    template(v-slot:body="props")
      q-tr(:props="props")
        q-td(key="title", :props="props")
          | {{ props.row.title }}
        q-td(key="author", :props="props")
          | {{ props.row.author }}
        q-td(key="image", :props="props")
          q-img(:src="`BooksCovers/${props.row.imageName}`" style="width: 150px; height:150px;")
        q-td(key="id", :props="props")
          q-btn(color='primary' icon='edit' @click="update(props.row.id), action='update'")
          q-btn(color='negative' icon='delete' @click="remove(props.row.id)")

  q-dialog(v-model='image_dialog')
    q-card
      q-card-section
        q-form()
        q-card-section
          text-h6(style="text-align:center;") Upload cover image
          q-uploader(ref='uploader' url='http://localhost:8000/api/upload' @uploaded="dialog = true, image_dialog = false")

  q-dialog(v-model='dialog')
    q-card
      q-card-section
        q-form(@submit='action')
          q-input(v-model='formData.title' label='Title' required='')
          q-input(v-model='formData.author' label='Author' required='')
          q-btn(color='primary' type='submit' label='finish')
          q-btn(color='negative' label='Cancel' @click='dialog = false')

  q-dialog(v-model='dialog2')
    q-card
      q-card-section
        q-form(@submit='updates')
          q-input(v-model='formData.title' label='Title' required='')
          q-input(v-model='formData.author' label='Author' required='')
          q-btn(color='primary' type='submit' label='finish')
          q-btn(color='negative' label='Cancel' @click='dialog2=false')
</template>


<script setup>
import { api } from "boot/axios"
import { useRouter } from "vue-router";
import { onMounted,reactive,ref} from "vue";


const update_value = ref();
const action = ref();
const dialog2 = ref(false);
const dialog = ref(false);
const image_dialog = ref(false);
const id = ref();
const books = ref();
const cols = reactive([
        { name: 'title', align: 'left', label: 'Title', field: 'title', sortable: true },
        { name: 'author', align: 'left', label: 'Author', field: 'author', sortable: true },
        { name: 'image', align: 'center', label: 'Image', field: 'imagePath', format: val => val ? 'Yes' : 'No' },
        { name: 'id', align: 'center', label: 'Actions' }
      ]);


let formData = ref({
        title: '',
        author: '',
        image: '',
      })

onMounted(() => {
  fetchBooks()
  latest_upload()
});

const fetchBooks = async () => {
  await api.get('/api/books')
        .then(response => {
          books.value = response.data;
        })
    }

const latest_upload = async ()=>{
  await api.get('/api/id')
        .then(response => {
          id.value = response.data.id;
          formData.value.image = response.data.ImagePath;
          console.log(response.data)
        })
}

const remove = async (id)=>{
  await api.delete(`/api/books/${id}`)
          .then(() => {
            fetchBooks()
          })
}
const create = async ()=>{
        await api.put(`/api/books/${id.value}`, formData.value)
          .then(() => {
            dialog.value = false
            fetchBooks()
          })
}
const updates = async ()=>{
  await api.put(`/api/books/${update_value.value}`, formData.value)
          .then(() => {
            fetchBooks()
            dialog2 = false;
          })
}

const update = async(id)=>{
    update_value.value = id;
    dialog2.value = true;
}
</script>

<style scoped>
.q-img {
  height: 50px;
}
</style>
